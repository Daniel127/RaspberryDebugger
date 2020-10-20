### Release Instructions

1. Switch to the release branch, creating one from **main** named like **release-v1.0** if necessary.

2. Merge **main** into the release branch.

3. Run the **SdkCatalogChecker** to validate the catalog.

4. Increment the version number and update:
   
   * `AssemblyInfo.cs:` both the assembly and file versions
   * `source.extension.vsixmanifest`

5. Update the release notes in: `ReleaseNotes.rtf` and `source.extension.vsixmanifest`

6. Open the solution, set the build configuration to **RELEASE** and then manually clean and build the solution.

7. Run this command to complete the release process, copying the build artifcats to the [$/Build] folder:

   `%RDBG_TOOLBIN%\builder.cmd`

8. Attach `$/Build/RaspberryDebugger.vsix** to the the release.

9. Copy/paste the SHA512 from `$/Build/RaspberryDebugger.vsix.sha512.txt** into the release notes.

10. Publish the GitHub release.

11. Commit any changes and push them to GitHub using a comment like: **RELEASE v1.0**

12. Switch back to the **main** branch, merge the changes from the release branch and push **main** to GitHub.

13. Create an .ZIP archive by executing:

    `%RDBG_TOOLBIN%\archive.cmd`

------------------------------------------------
$todo(jefflill): Flesh these out:

14. Sign the extension??

15. Release to Visual Studio Marketplace??
------------------------------------------------

### Post Release Steps

1. Create the next release branch from main named like: release-v1.0" and push it to GitHub.

2. Create a new GitHub release with tag like v1.0 and named like v1.0 and select the next release branch.  Copy `RELEASE-TEMPLATE.md` as the initial release description.
