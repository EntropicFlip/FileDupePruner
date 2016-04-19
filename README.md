# FileDupePruner
C# Tool using Windows Forms that recursively detects and optionally moves duplicate files.

Duplicate ignores filename and uses binary comparison, instead.
Duplicates are moved to a _Pruned folder for inspection by a human before deletion.

Typical usage pattern would be to prune duplicate photos and videos from cloud account services such as dropbox, which duplicates files on a camera phone if the user chooses to use the auto-upload feature. Other automated services might also create copies of files for organizational purposes.
