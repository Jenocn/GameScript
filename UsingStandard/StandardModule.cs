using gs;
namespace std {
    public class StandardModule : VMModuleBase {
        public override bool OnModuleLoad() {
            VM.AddUsing("std.file", new std.file.FileUsing());
            VM.AddUsing("std.console", new std.console.ConsoleUsing());
            VM.AddUsing("std.module", new std.module.ModuleUsing());
            VM.AddUsing("std.list", new std.list.ListUsing());
            VM.AddUsing("std.string", new std.str.StringUsing());
            return true;
        }
    }
}