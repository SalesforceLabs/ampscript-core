// Copyright (c) 2023, salesforce.com, inc.
// All rights reserved.
// SPDX-License-Identifier: Apache-2.0
// For full license text, see the LICENSE file in the repo root or https://opensource.org/licenses/Apache-2.0

import { config } from 'process';
import * as vscode from 'vscode';
import * as path from 'path';
import * as assets from './assets';

let extensionContext: vscode.ExtensionContext;
export default function registerCommands(context: vscode.ExtensionContext) {
    let workspaceFolder = vscode.workspace.workspaceFolders?.at(0);
    extensionContext = context;
    if (workspaceFolder === undefined)
    {
        return;
    }

    const generator = new assets.AssetGenerator(context, workspaceFolder);

	// The command has been defined in the package.json file
	// Now provide the implementation of the command with registerCommand
	// The commandId parameter must match the command field in package.json
	// context.subscriptions.push(vscode.commands.registerCommand('ampscript.generateLaunchJson', async () => generateLaunchJson(generator)));
	context.subscriptions.push(vscode.commands.registerCommand('ampscript.getWebhostLocation', () => getWebhostLocation()));
}

async function generateLaunchJson(generator: assets.AssetGenerator) {
    await assets.addLaunchJsonIfNecessary(generator);
}

function getWebhostLocation() {
    return extensionContext.asAbsolutePath(path.join(".Sage", "Sage.Webhost.dll"));
}