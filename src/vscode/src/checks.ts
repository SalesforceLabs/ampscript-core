/* eslint-disable @typescript-eslint/naming-convention */
import * as vscode from 'vscode';

export function extensionsChecks() {
	const extensionHandlers = {
		"sergey-agadzhanov.AMPscript": {
			handler: () => {
				vscode.window.showInformationMessage(`MCFS [AMPScript] extension is currently not supported by AMPscript Core.`);
				return false;
			}
		},
		"FiB.beautyAmp": {
			handler: () => true
		}
	};

	let allSupported = true;
	for (let extensionId in extensionHandlers) {
		const extension = vscode.extensions.getExtension(extensionId);
		if (extension) {
			const handler = extensionHandlers[extensionId as keyof typeof extensionHandlers].handler;
			if (handler) {
				const result = handler();
				if (result === false) {
					allSupported = false;
				}
			}
		}
	}
	return allSupported;
}