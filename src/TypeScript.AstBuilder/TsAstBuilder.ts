'use strict';
import * as Path from 'path';
import * as fs from 'fs';
import * as typescript from 'typescript';

namespace tools {
    export class TsAstBuilderProject {

        private sourcePath: string;
        private outputFolder: string;

        constructor(source: string, output: string) {
            this.sourcePath = Path.resolve(__dirname, source);
            this.outputFolder = Path.resolve(__dirname, output);
        }

        public run(): void {
            if (fs.existsSync(this.sourcePath)) {
                this.printLine('Start parsing the typescript files...');

                if (fs.existsSync(this.outputFolder)) {
                    this.removeFolder(this.outputFolder);
                }

                if (fs.statSync(this.sourcePath).isDirectory()) {
                    this.visitFolder(this.sourcePath)
                } else {
                    this.visitFile(this.sourcePath);
                }

                this.printLine('Finishing parsing.');
            }
            else {
                this.printLine('Cannot find the typescript files folder ' + this.sourcePath);
            }
        }

        protected visitFolder(path: string): void {
            var items = fs.readdirSync(path);

            for (let item of items) {
                let itemPath = Path.join(path, item);
                if (fs.statSync(itemPath).isDirectory()) {
                    this.visitFolder(itemPath);
                } else {
                    this.visitFile(itemPath);
                }
            }
        }
        protected visitFile(path: string): void {
            if (Path.extname(path) == '.ts') {
                let astFilePath = Path.join(this.outputFolder, Path.relative(this.sourcePath, path)) + '.json';
                let astFileFolder = Path.dirname(astFilePath);
                this.makeFolder(astFileFolder);

                let builder = new TsAstBuilder();
                let ast = builder.build(path);
                fs.writeFile(astFilePath, JSON.stringify(ast, null, 2), (error) => { if (error != null) { throw error; } });
            }
        }
        protected printLine(message: string): void {
            console.log('[' + new Date().toLocaleTimeString() + ']: ' + message);
        }

        private removeFolder(path: string): void {
            if (fs.existsSync(path)) {
                let items = fs.readdirSync(path);
                for (let item of items) {
                    let itemPath = Path.join(path, item);
                    if (fs.statSync(itemPath).isDirectory()) {
                        this.removeFolder(itemPath);
                    } else {
                        fs.unlinkSync(itemPath);
                    }
                }
            }
        }
        private makeFolder(path: string): void {
            if (fs.existsSync(path)) {
                return;
            }

            let parent = Path.dirname(path);
            this.makeFolder(parent);
            fs.mkdirSync(path);
        }
    }


    export class TsAstBuilder {
        public build(fileName: string): object {
            let sourceFileNode = typescript.createSourceFile(fileName, fs.readFileSync(fileName).toString(), typescript.ScriptTarget.ES2015, true);

            return this._toJson(sourceFileNode);
        }

        protected _toJson(node: typescript.Node): object {
            let json: { [key: string]: any } = {};
            for (let key in node) {
                if (key == 'parent') {
                    continue;
                } else if (key == 'kind') {
                    json['kind'] = typescript.SyntaxKind[node.kind].toString();
                }
                else {
                    let value = (node as any)[key];
                    if ((value == null) ||
                        (typeof value === 'function')) {
                    } else if ((typeof value === 'string') ||
                        (typeof value === 'boolean') ||
                        (typeof value === 'number')) {
                        json[key] = value;
                    } else if (Array.isArray(value)) {
                        json[key] = [];
                        for (let item of value) {
                            if ((typeof item === 'string') ||
                                (typeof item === 'boolean') ||
                                (typeof item === 'number')) {
                                json[key].push(item);
                            } else {
                                json[key].push(this._toJson(item));
                            }
                        }
                    } else if (typeof value === 'object') {
                        json[key] = this._toJson(value);
                    } else {
                        console.log(key + ':' + value + ':');
                        throw 'Unexcepted type is found!';
                    }
                }
            }
            return json;
        }
    }
}

export const TsAstBuilder = tools.TsAstBuilder;
export const TsAstBuilderProject = tools.TsAstBuilderProject;