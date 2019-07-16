var vscode = require('vscode');
function activate(context) {
    console.log('AncientAssembly extension activated!');
    console.log("path " + vscode.workspace.rootPath);
}
function deactivate() { }
exports.deactivate = deactivate;
exports.activate = activate;