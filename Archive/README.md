# Archive Folder

This folder contains historical files, old reports, and deprecated documentation that are no longer actively used in development but are preserved for reference.

## Structure

```
Archive/
├── Reports/                # Sprint reports and completion summaries
├── Old-Files/              # Temporary and deprecated files
├── Documentation/          # Outdated or sprint-specific documentation
└── Deployment-Archive/     # Historical deployment documentation and scripts
```

## Contents

### Reports/
Historical sprint reports and deployment completion summaries:
- Sprint N06 reports (bug fixes, completion, deliverables, frontend improvements)
- Deployment reports (Nov 2025)
- Bug fix reports
- Vault restoration reports

### Old-Files/
Deprecated and temporary files no longer in use:
- `.gitignore.tmp` - Old temporary gitignore file
- `nul` - Empty placeholder file
- `token.json` - Old authentication token file
- `cleanup-testing-folder.ps1` - Old cleanup script

### Documentation/
Sprint-specific or outdated documentation:
- `CACHING_QUICK_REFERENCE.md` - Response caching guide (sprint-specific)
- `CRITICAL_BUGS_FIXES_NEEDED.md` - Historical bug tracking (issues resolved)
- `IMPLEMENTATION_GUIDE.md` - Old implementation guide
- `RESPONSE_CACHING_SUMMARY.md` - Caching implementation summary
- `XML_DOCUMENTATION_STANDARDS.md` - XML documentation standards

### Deployment-Archive/
Historical deployment-related files from Deployment folder:
- Old deployment guides and summaries
- Test scripts and SQL files
- Port change documentation
- Manual review summaries
- Testnet guides

## When to Archive Files

Files should be moved to Archive when they are:
1. **Historical Reports** - Sprint completion reports, old summaries
2. **Deprecated Documentation** - Replaced by newer guides or no longer relevant
3. **Temporary Files** - Old temporary files no longer needed
4. **Completed Tasks** - Documentation for completed sprints or resolved issues
5. **Superseded Scripts** - Old scripts replaced by newer versions

## When to Keep Files in Root

Files should stay in the root when they are:
1. **Active Development** - Currently used in daily development
2. **Configuration** - Docker, environment, git configuration
3. **Core Documentation** - README.md, current guides
4. **Source Code** - All project source folders
5. **Current Scripts** - Actively used deployment/build scripts

## Retrieval

If you need to reference archived files:
1. Navigate to the appropriate Archive subdirectory
2. Files are organized by type for easy lookup
3. All files are preserved with original timestamps

## Maintenance

This folder is periodically reviewed to:
- Ensure proper organization
- Remove truly obsolete files (after 1+ year)
- Consolidate similar historical documents
- Update this README with new archive categories

---

Last Updated: 2025-11-07
