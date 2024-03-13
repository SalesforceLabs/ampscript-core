/* eslint-disable @typescript-eslint/naming-convention */
import * as vscode from 'vscode';

export function extensionsChecks() {
	const extensionHandlers = {
		// "publisher.extensionName":
		"sergey-agadzhanov.AMPscript": {
			// handler returns false if the extension is incompatible, true otherwise.
			// handler can also run additional actions in the case there's a need.
			handler: () => {
				vscode.window.showInformationMessage(`The extension MCFS [AMPScript] is incompatible with AMPscript Core and will prevent AMPscript Core from debugging AMPscript.`);
				return false;
			}
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