namespace gs.std {
    public class StandardModule : VMModuleBase {
        public override bool OnModuleLoad() {
            VM.AddUsing("std.file", new file.FileUsing());
            VM.AddUsing("std.console", new console.ConsoleUsing());
            VM.AddUsing("std.module", new module.ModuleUsing());
            VM.AddUsing("std.list", new list.ListUsing());
            VM.AddUsing("std.string", new str.StringUsing());
            return true;
        }
    }
}