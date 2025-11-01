import { Realm, type ObjectSchema } from "realm";
import { existsSync } from "fs";
import path from "path";

const PRIMITIVE_TYPES = ["int", "float", "double", "bool", "string", "date", "data"];

export function validatePath(givenPath: string) {
    if (givenPath.startsWith('"') || givenPath.endsWith('"')) {
        return Error(`The path must not start or end with \`"\``);
    }

    if (!existsSync(givenPath)) {
        return Error(`The path ${givenPath} does not exist.`);
    }

    if (!existsSync(givenPath)) {
        return Error(`The path ${givenPath} does not exist. Please double check it.`);
    }

    const ext = path.extname(givenPath);
    if (ext !== ".realm") {
        return Error(`The given file is not a .realm file. Please double check it.`);
    }

    return;
}

export function getPrimaryKey(schemaName: string, realm: Realm): string | undefined {
    return realm.schema.find((s) => s.name === schemaName)?.primaryKey;
}

export function findExistingByOnlineId(schemaName: string, onlineId: number, realm: Realm): any {
    return realm.objects(schemaName).filtered("OnlineID == $0", onlineId);
}

export function createShallowCopy(obj: any, schema: ObjectSchema, sourceRealm: Realm, targetRealm: Realm): any {
    const data: any = {};

    for (const [propName, propDef] of Object.entries(schema.properties)) {
        const value = obj[propName];

        if (value === null || value === undefined) {
            data[propName] = value;
            continue;
        }

        if (typeof propDef === "object" && propDef.type === "object") {
            const refSchema = sourceRealm.schema.find((s) => s.name === propDef.objectType);
            const refPrimaryKey = refSchema?.primaryKey;

            if (refPrimaryKey) {
                const refPkValue = value[refPrimaryKey];
                const existingRef = targetRealm.objectForPrimaryKey(propDef.objectType!, refPkValue);

                if (existingRef) {
                    data[propName] = existingRef;
                } else {
                    data[propName] = null;
                }
            } else {
                data[propName] = value;
            }
        } else if (typeof propDef === "object" && (propDef.type === "list" || propDef.type === "set")) {
            if (propDef.objectType && !["int", "float", "double", "bool", "string", "date", "data"].includes(propDef.objectType)) {
                const refSchema = sourceRealm.schema.find((s) => s.name === propDef.objectType);
                const refPrimaryKey = refSchema?.primaryKey;

                if (refPrimaryKey) {
                    const existingRefs = [];
                    for (const item of value) {
                        const refPkValue = item[refPrimaryKey];
                        const existingRef = targetRealm.objectForPrimaryKey(propDef.objectType, refPkValue);
                        if (existingRef) {
                            existingRefs.push(existingRef);
                        }
                    }
                    data[propName] = existingRefs;
                } else {
                    data[propName] = Array.from(value).map((item: any) => {
                        const embeddedSchema = sourceRealm.schema.find((s) => s.name === propDef.objectType);
                        if (embeddedSchema) {
                            return createShallowCopy(item, embeddedSchema, sourceRealm, targetRealm);
                        }
                        return item;
                    });
                }
            } else {
                data[propName] = Array.from(value);
            }
        } else {
            data[propName] = value;
        }
    }

    return data;
}
