# Git Commands Reference - flex_book Project

This document contains all the Git commands used to set up and manage the flex_book repository.

## Initial Repository Setup

### 1. Initialize Git Repository
```bash
git init
```
Creates a new Git repository in the current directory.

### 2. Configure User Information
```bash
git config user.name "Roger Nie"
git config user.email "roger.nie.net@gmail.com"
```
Sets your identity for commits in this repository.

### 3. Stage All Files
```bash
git add .
```
Adds all files in the current directory to the staging area.

### 4. Create Initial Commit
```bash
git commit -m "Initial commit: Modern .NET 8 WPF Desktop Application"
```
Creates the first commit with all staged files.

### 5. Add Remote Repository
```bash
git remote add origin https://github.com/RogerNieGit/flex_book.git
```
Links your local repository to the GitHub remote repository.

### 6. Rename Branch to Main
```bash
git branch -M main
```
Renames the default branch from 'master' to 'main' (GitHub's current standard).

### 7. Create GitHub Repository (using GitHub CLI)
```bash
gh repo create RogerNieGit/flex_book --public --source=. --remote=origin --push
```
Creates the repository on GitHub and attempts to push the code.

### 8. Push to GitHub
```bash
git push -u origin main
```
Pushes your local main branch to GitHub and sets up tracking.

---

## Common Git Commands for Daily Use

### Check Repository Status
```bash
git status
```
Shows which files have been modified, staged, or are untracked.

### View Commit History
```bash
git log
git log --oneline        # Compact view
git log --graph --all    # Visual branch history
```

### Stage Specific Files
```bash
git add filename.txt
git add *.cs             # Stage all C# files
```

### Commit Changes
```bash
git commit -m "Your commit message here"
git commit -am "Stage and commit in one step"  # Only for tracked files
```

### Push Changes to GitHub
```bash
git push
git push origin main     # Explicit branch name
```

### Pull Changes from GitHub
```bash
git pull
git pull origin main     # Explicit branch name
```

### View Remote Repository
```bash
git remote -v            # View remote URLs
git remote show origin   # Detailed remote info
```

### Create a New Branch
```bash
git branch feature-name
git checkout -b feature-name    # Create and switch to new branch
```

### Switch Branches
```bash
git checkout branch-name
git switch branch-name          # Modern alternative
```

### Merge Branches
```bash
git checkout main
git merge feature-name
```

### View Differences
```bash
git diff                 # Unstaged changes
git diff --staged        # Staged changes
git diff HEAD            # All changes since last commit
```

### Undo Changes
```bash
# Discard changes in working directory
git checkout -- filename.txt

# Unstage a file (keep changes)
git reset HEAD filename.txt

# Undo last commit (keep changes)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1
```

### View .gitignore Status
```bash
git check-ignore -v filename    # Check if file is ignored
```

---

## Project-Specific Information

### Repository Details
- **Repository URL:** https://github.com/RogerNieGit/flex_book
- **User Name:** Roger Nie
- **User Email:** roger.nie.net@gmail.com
- **Default Branch:** main

### Initial Commit Summary
- **Commit Hash:** 73f1c38
- **Files Committed:** 10 files
- **Lines of Code:** 523 insertions
- **Message:** "Initial commit: Modern .NET 8 WPF Desktop Application"

### Files Included in Repository
1. .gitignore
2. App.xaml
3. App.xaml.cs
4. AssemblyInfo.cs
5. MainWindow.xaml
6. MainWindow.xaml.cs
7. ModernDesktopApp.csproj
8. README.md
9. flex_book.bat
10. flex_book.sln

---

## Workflow Tips

### Daily Development Workflow
```bash
# 1. Check status
git status

# 2. Stage your changes
git add .

# 3. Commit with meaningful message
git commit -m "Add new feature: description"

# 4. Push to GitHub
git push

# 5. Pull latest changes (before starting work)
git pull
```

### Before Starting New Work
```bash
git pull                 # Get latest changes
git status              # Ensure clean working directory
```

### Good Commit Messages
- ✅ "Add user authentication feature"
- ✅ "Fix button click event handler"
- ✅ "Update README with installation instructions"
- ❌ "changes"
- ❌ "fixed stuff"
- ❌ "asdf"

---

## Troubleshooting

### If Remote Already Exists
```bash
git remote remove origin
git remote add origin https://github.com/RogerNieGit/flex_book.git
```

### If Push is Rejected
```bash
# Pull changes first, then push
git pull origin main --rebase
git push origin main
```

### If You Need to Change Last Commit Message
```bash
git commit --amend -m "New commit message"
git push --force  # Use with caution!
```

---

## Additional Resources

- [Git Official Documentation](https://git-scm.com/doc)
- [GitHub Guides](https://guides.github.com/)
- [Pro Git Book](https://git-scm.com/book/en/v2)
- [GitHub CLI Documentation](https://cli.github.com/manual/)

---

**Last Updated:** January 28, 2026  
**Repository:** https://github.com/RogerNieGit/flex_book