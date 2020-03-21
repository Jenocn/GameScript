using System;
using gs;
using std.io;
using std.console;
namespace std {
    public class StandardModule : VMModuleBase {
        public override bool OnModuleLoad() {
            VM.AddUsing("std.io", new IOUsing());
            VM.AddUsing("std.console", new ConsoleUsing());
            return true;
        }
    }
}