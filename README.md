# FileDupePruner
C# Tool using Windows Forms that recursively detects and optionally moves duplicate files.

This tool ignores filenames and uses binary comparisons, instead.
Duplicates are moved to a _Pruned folder for inspection by a human before deletion.

Typical usage pattern would be to prune duplicate photos and videos from cloud account services such as dropbox, which duplicates files on a camera phone if the user chooses to use the auto-upload feature. Other automated services might also create copies of files for organizational purposes.

This tool will identify those duplicates and give you the option to move them to another folder that you can manually delete after inspection.

You can either evaluate two folders, leaving the primary folder intact, or you can evaluate a single folder for any duplicates within itself. All folder evaluations are recursive.
