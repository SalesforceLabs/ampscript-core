// The module 'vscode' contains the VS Code extensibility API
// Import the module and reference it with the alias vscode in your code below
import * as vscode from 'vscode';
import * as path from 'path';
import TelemetryReporter from '@vscode/extension-telemetry';
import registerCommands from './commands';

// the application insights key (also known as instrumentation key)
const applicationInsightsKey = 'd3cdabd2-8a47-44c7-92bd-7c786d2dd3bd';
let reporter: TelemetryReporter;

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
    reporter = new TelemetryReporter(applicationInsightsKey);

    registerCommands(context);

    // register a configuration provider for 'mock' debug type
    const provider = new AmpscriptConfigurationProvider(context);
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider('ampscript', provider));
    context.subscriptions.push(vscode.debug.registerDebugConfigurationProvider('JSON', provider));
    context.subscriptions.push(reporter);
}

// This method is called when your extension is deactivated
export function deactivate() {}

export class AmpscriptConfigurationProvider implements vscode.DebugConfigurationProvider {
    
    public extensionContext: vscode.ExtensionContext;

    public constructor(context: vscode.ExtensionContext) {
        this.extensionContext = context;
    }

    /**
     * Massage a debug configuration just before a debug session is being launched,
     * e.g. add all missing attributes to the debug configuration.
     */
    public async resolveDebugConfiguration(folder: vscode.WorkspaceFolder | undefined, config: vscode.DebugConfiguration, token?: vscode.CancellationToken): Promise<vscode.DebugConfiguration | undefined>
    {
        var cwd = folder === undefined ? "${fileDirname}" : folder.uri.fsPath;
        // if launch.json is missing or empty
        if (!config.type && !config.request && !config.name) {
            const editor = vscode.window.activeTextEditor;

            if (!editor) {
                return;
            }
            config.type = 'coreclr';
            config.name = 'Launch AMPscript file';
            config.request = 'launch';
            config.program = `${this.extensionContext.asAbsolutePath(path.join(".Sage", "Sage.Webhost.dll"))}`;
            config.cwd = cwd;
            config.stopAtEntry = false;
            config.serverReadyAction = {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            };
            config.env = {
                "ASPNETCORE_ENVIRONMENT": "Development"
            };
            config.sourceFileMap = {
                "/Views":  cwd
            };

            if (editor.document.languageId === 'json') {
                let args = await this.selectPackageManagerPackage(editor);
                if (args === undefined) {
                    return undefined;
                }
                
                config.args = args;
            } else if (editor.document.languageId === 'ampscript') {
                config.args = "ampscript --source ${file}";
                reporter.sendTelemetryEvent("launch", {'type': 'ampscriptfile'});
            }
        }

        return config;
    }

    public async selectPackageManagerPackage(editor: vscode.TextEditor) {
        const documentText = editor.document.getText();

        let documentObj = JSON.parse(documentText);

        let assetMap = undefined;

        let returnArguments = "";

        // JSON packages have a 'modelVersion' in the root
        if (documentObj["modelVersion"] === '4') {
            returnArguments += "packagejson --source \"${file}\" --sourceId ";
            assetMap = documentObj["selectedEntities"]["assets"];
            
            reporter.sendTelemetryEvent("launch", {'type': 'packagejson'});
        } else {
            // ZIP packages only support auto-running from the 'selected-entities.json' file.
            returnArguments += "packagezip --packageDir \"${fileDirname}\" --sourceId ";
            assetMap = documentObj["assets"];
            reporter.sendTelemetryEvent("launch", {'type': 'packagezip'});
        }

        if (assetMap === undefined) {
            return undefined;
        }
        
        let assetNames = assetMap;

        try {
            if (assetNames.length >= 1) {
                var chosenAssetId = undefined;
                if (assetNames.length === 1) {
                    chosenAssetId = assetNames[0];
                } else {
                    chosenAssetId = await vscode.window.showQuickPick(assetNames.map((a : number) => a.toString()));
                }

                returnArguments += chosenAssetId;
        
                return returnArguments;
            }
        } catch (e) {}
        vscode.window.showInformationMessage("No assets found in PackageManager file");
        return undefined;
    }
}