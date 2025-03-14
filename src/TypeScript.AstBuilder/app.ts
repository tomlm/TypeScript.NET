#!/usr/bin/env node
import { TsAstBuilder, TsAstBuilderProject } from './TsAstBuilder';

'use strict';

namespace tools {
    export class Program {
        public static main(args: string[]): void {
            if (args.length != 2) {
                Program.showUsage();
            } else {
                let application = new TsAstBuilderProject(args[0], args[1])
                application.run();
            }
        }
        public static showUsage(): void {
            console.log();
            console.log('Usage: ts2ast [file>|folder] [output folder]');
            console.log();
        }

    }
}

tools.Program.main(process.argv.slice(2));


