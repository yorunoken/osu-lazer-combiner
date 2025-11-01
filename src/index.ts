import Realm from "realm";
import { text, isCancel, cancel, confirm, intro, log, outro } from "@clack/prompts";
import { validatePath } from "./util";
import { mergeRealms } from "./merger";

console.log("This is an app that will let you merge two `client.realm` files from different osu!lazer installs, allowing you the new osu!lazer install to recognize the files from the old install.");
console.log();
intro("osu!lazer combiner");
const sourcePath = await text({ message: "The path of your source client.realm file. Data will be copied over from this over to the target file.", validate: validatePath });

if (isCancel(sourcePath)) {
    cancel("Cancelled.");
    process.exit(0);
}

const targetPath = await text({ message: "The path of your target client.realm file. Data will be copied into this file from the source file.", validate: validatePath });

if (isCancel(targetPath)) {
    cancel("Cancelled.");
    process.exit(0);
}

const wantsLogs = await confirm({ message: "Do you want logs to be displayed?", initialValue: false });

if (isCancel(wantsLogs)) {
    cancel("Cancelled.");
    process.exit(0);
}

const ok = await confirm({ message: `Before we start, do you confirm that you've made backups of your files?`, initialValue: false });
if (isCancel(ok)) {
    cancel("Cancelled.");
    process.exit(0);
}

if (!ok) {
    log.error("PLEASE.... Make backups before attempting this.");
    process.exit(0);
}

log.info(`Starting merge process. ${!wantsLogs && "We'll let you know when it finishes :)"}`);

const sourceRealm = await Realm.open(sourcePath);
const targetRealm = await Realm.open(targetPath);

await mergeRealms(sourceRealm, targetRealm, wantsLogs);

sourceRealm.close();
targetRealm.close();

log.success(
    `Successfully merged files. You can now copy "${targetPath}" into your new osu!lazer install. Please do not remove your backups until you've confirmed osu! works with no issues, and you can still access your beatmaps and scores.`
);
