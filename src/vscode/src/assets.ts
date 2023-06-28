
import * as fs from 'fs-extra';
import * as jsonc from 'jsonc-parser';
import { FormattingOptions, ModificationOptions } from 'jsonc-parser';
import * as os from 'os';
import * as path from 'path';
import * as vscode from 'vscode';

export class AssetGenerator {
    public vscodeFolder: string;
    public launchJsonPath: string;
    public extensionContext: vscode.ExtensionContext;

    public constructor(context: vscode.ExtensionContext, private workspaceFolder: vscode.WorkspaceFolder) {
        this.vscodeFolder = path.join(this.workspaceFolder.uri.fsPath, '.vscode');
        this.launchJsonPath = path.join(this.vscodeFolder, 'launch.json');
        this.extensionContext = context;
    }
    
    public createLaunchJsonConfigurations(): string {
        const configuration = {
            "name": "Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "program": `${this.extensionContext.asAbsolutePath(path.join(".Sage", "Sage.Webhost.dll"))}`,
            "cwd": "\${workspaceFolder}",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "\${workspaceFolder}"
            }
        };
    
        return JSON.stringify([configuration]);
    }

}

export function getFormattingOptions(): FormattingOptions {
    const editorConfig = vscode.workspace.getConfiguration('editor');

    const tabSize = editorConfig.get<number>('tabSize') ?? 4;
    const insertSpaces = editorConfig.get<boolean>('insertSpaces') ?? true;

    const filesConfig = vscode.workspace.getConfiguration('files');
    const eolSetting = filesConfig.get<string>('eol');
    const eol = !eolSetting || eolSetting === 'auto' ? os.EOL : '\n';

    const formattingOptions: FormattingOptions = {
        insertSpaces: insertSpaces,
        tabSize: tabSize,
        eol: eol
    };

    return formattingOptions;
}

export async function addLaunchJsonIfNecessary(generator: AssetGenerator) {
    return new Promise<void>(() => {
        if (fs.pathExistsSync(generator.launchJsonPath)) {
            return;
        }

        const launchJsonConfigurations: string = generator.createLaunchJsonConfigurations();
        const formattingOptions = getFormattingOptions();

        let text: string;
        // when launch.json does not exist, create it and write all the content directly
        const launchJsonText = `
        {
            "version": "0.2.0",
            "configurations": ${launchJsonConfigurations}
        }`;

        text = jsonc.applyEdits(launchJsonText, jsonc.format(launchJsonText, undefined, formattingOptions));

        fs.ensureDir(generator.vscodeFolder, err => {
            fs.writeFile(generator.launchJsonPath, text, err => {
                if (err) {
                    vscode.window.showErrorMessage(err.message);
                }
            });
        });
    });
}