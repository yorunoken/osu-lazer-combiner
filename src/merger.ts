import Realm from "realm";
import { createShallowCopy, findExistingByOnlineId, getPrimaryKey } from "./util";
import { log } from "@clack/prompts";

const schemasToMerge = ["Skin", "Ruleset", "ModPreset", "File", "BeatmapMetadata", "Beatmap", "BeatmapSet", "BeatmapCollection", "Score"] as const;

export async function mergeRealms(sourceRealm: Realm.Realm, targetRealm: Realm.Realm, wantsLogs: boolean) {
    // FIRST PASS: Create objects without their relationships
    // This prevents errors when objects reference each other
    if (wantsLogs) log.info("=== Creating objects ===\n");

    for (const schemaName of schemasToMerge) {
        if (wantsLogs) log.step(`Processing schema: ${schemaName}...`);

        const sourceObjects = sourceRealm.objects(schemaName);
        const totalCount = sourceObjects.length;
        const sourceSchema = sourceRealm.schema.find((s) => s.name === schemaName);
        const primaryKey = sourceSchema?.primaryKey;

        let copiedCount = 0;
        let skippedCount = 0;

        targetRealm.write(() => {
            for (const sourceObject of sourceObjects) {
                // skip if object with same OnlineID already exists
                if ("OnlineID" in sourceObject && sourceObject.OnlineID !== -1) {
                    const matchingObjects = targetRealm.objects(schemaName).filtered("OnlineID == $0", sourceObject.OnlineID);
                    if (matchingObjects.length > 0) {
                        skippedCount++;
                        continue;
                    }
                }

                // create directly if object has no primary key
                if (!primaryKey) {
                    const objectData = createShallowCopy(sourceObject, sourceSchema!, sourceRealm, targetRealm);
                    targetRealm.create(schemaName, objectData);
                    copiedCount++;
                    continue;
                }

                // skip if object with same primary key already exists
                const primaryKeyValue = sourceObject[primaryKey] as string;
                const existingObject = targetRealm.objectForPrimaryKey(schemaName, primaryKeyValue);

                if (existingObject) {
                    skippedCount++;
                    continue;
                }

                // create new object in target realm
                const objectData = createShallowCopy(sourceObject, sourceSchema!, sourceRealm, targetRealm);
                targetRealm.create(schemaName, objectData, Realm.UpdateMode.Modified);
                copiedCount++;
            }
        });

        if (wantsLogs) log.success(`${schemaName}: ${copiedCount} copied, ${skippedCount} skipped (${totalCount} total)\n`);
    }

    // Link objects to each other
    if (wantsLogs) log.info("=== SECOND PASS: Updating relationships ===\n");

    for (const schemaName of schemasToMerge) {
        if (wantsLogs) log.step(`Updating relationships for schema ${schemaName}...`);

        const sourceObjects = sourceRealm.objects(schemaName);
        const sourceSchema = sourceRealm.schema.find((s) => s.name === schemaName);
        const primaryKey = sourceSchema?.primaryKey;

        // skip schemas without primary keys (can't reliably match objects)
        if (!primaryKey) {
            if (wantsLogs) log.warn(`${schemaName}: No primary key, skipping relationship update\n`);
            continue;
        }

        let updatedCount = 0;

        targetRealm.write(() => {
            for (const sourceObject of sourceObjects) {
                // skip duplicates
                if ("OnlineID" in sourceObject && sourceObject.OnlineID !== -1) {
                    const matchingObjects = findExistingByOnlineId(schemaName, sourceObject.OnlineID as number, targetRealm);
                    if (matchingObjects.length > 1) continue;
                }

                const primaryKeyValue = sourceObject[primaryKey] as string;
                const targetObject = targetRealm.objectForPrimaryKey(schemaName, primaryKeyValue);

                if (!targetObject) continue;

                // update each property's relationships
                for (const [propertyName, propertyDefinition] of Object.entries(sourceSchema!.properties)) {
                    if (typeof propertyDefinition === "object" && propertyDefinition.type === "object") {
                        const sourceValue = sourceObject[propertyName] as any;

                        if (sourceValue && propertyDefinition.objectType) {
                            const refSchema = sourceRealm.schema.find((s) => s.name === propertyDefinition.objectType);
                            const refPrimaryKey = refSchema?.primaryKey;

                            if (refPrimaryKey) {
                                const refPkValue = sourceValue[refPrimaryKey];
                                let existingRef = targetRealm.objectForPrimaryKey(propertyDefinition.objectType, refPkValue);

                                if (!existingRef && "OnlineID" in sourceValue && sourceValue.OnlineID !== -1) {
                                    const refsByOnlineId = targetRealm.objects(propertyDefinition.objectType).filtered("OnlineID == $0", sourceValue.OnlineID);
                                    if (refsByOnlineId.length > 0) {
                                        existingRef = refsByOnlineId[0] || null;
                                    }
                                }

                                if (existingRef) {
                                    (targetObject as any)[propertyName] = existingRef;
                                } else if ((targetObject as any)[propertyName] === null) {
                                    if (wantsLogs) log.warn(`${schemaName}.${propertyName}: Reference ${propertyDefinition.objectType}[${refPkValue}] not found`);
                                }
                            }
                        }
                    }

                    if (typeof propertyDefinition === "object" && (propertyDefinition.type === "list" || propertyDefinition.type === "set")) {
                        const sourceValue = sourceObject[propertyName] as any;

                        if (propertyDefinition.objectType && !["int", "float", "double", "bool", "string", "date", "data"].includes(propertyDefinition.objectType)) {
                            const refSchema = sourceRealm.schema.find((s) => s.name === propertyDefinition.objectType);
                            const refPrimaryKey = refSchema?.primaryKey;

                            if (refPrimaryKey) {
                                const existingRefs = [];
                                for (const item of sourceValue) {
                                    const refPkValue = item[refPrimaryKey];
                                    const existingRef = targetRealm.objectForPrimaryKey(propertyDefinition.objectType, refPkValue);
                                    if (existingRef) {
                                        existingRefs.push(existingRef);
                                    }
                                }

                                (targetObject as any)[propertyName].splice(0, (targetObject as any)[propertyName].length, ...existingRefs);
                            }
                        }
                    }
                }

                updatedCount++;
            }
        });

        if (wantsLogs) log.success(`${schemaName}: ${updatedCount} relationships updated\n`);
    }
}
