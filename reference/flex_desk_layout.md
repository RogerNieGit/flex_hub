# FlexDesk Application Layout Guide

This document outlines the FlexDesk application's layout structure to help developers understand and recreate the UI architecture.

## Overview

FlexDesk is a desktop application built with Tauri + Vue 3 featuring a modular layout with:
- Top menu bar
- Left docker bar with tool icons
- Central main area
- Right sidebar/side panel
- Window maximized by default

## Layout Hierarchy

```
┌─────────────────────────────────────────────────────────────┐
│ MenuBar (35px height)                                       │
├──────┬────────────────────────────────────────┬─────────────┤
│      │                                        │             │
│Docker│          Main Area                     │  Sidebar    │
│ Bar  │         (Flexible)                     │  (300px)    │
│(48px)│                                        │             │
│      │  ┌──────────────────────────────────┐  │             │
│      │  │ Top Header (varies by tool)      │  │             │
│      │  ├──────────────────────────────────┤  │             │
│      │  │ Title Header (fixed, 60-80px)    │  │             │
│      │  ├──────────────────────────────────┤  │             │
│      │  │                                  │  │             │
│      │  │ Content Area (flexible)          │  │             │
│      │  │                                  │  │             │
│      │  └──────────────────────────────────┘  │             │
│      │                                        │             │
└──────┴────────────────────────────────────────┴─────────────┘
```

## Component Dimensions

### 1. Menu Bar
- **Height**: `35px` (fixed)
- **Background**: `#3c3c3c`
- **Border**: `1px solid #2b2b2b` (bottom)
- **Features**: Draggable window region with File, Edit, View, Help menus

### 2. Docker Bar (Left)
- **Width**: `48px` (fixed)
- **Background**: `#2b2b2b`
- **Border**: `1px solid #1e1e1e` (right)
- **Icon Size**: `24px`
- **Icon Container**: `48px × 48px`
- **Features**:
  - Top icons: Dynamic tool items
  - Bottom icons: Profile and Settings (fixed)
  - Active indicator: `2px` blue line on left edge
  - Hover effect: `#3c3c3c` background

### 3. Main Area (Center)
- **Width**: Flexible (fills remaining space)
- **Structure**:

#### Top Header (Optional - varies by tool)
- **Height**: Variable (e.g., 50-60px for Audio Player settings button area)
- **Background**: `#252525`
- **Border**: `1px solid #333` (bottom)
- **Content**: Tool-specific controls (e.g., Settings button)

#### Title Header (Fixed Header in Main Area)
- **Height**: `60-80px` (typically 60px minimum)
- **Background**: `#2d2d30`
- **Border**: `3px solid #4a9eff` (bottom accent)
- **Layout**: Horizontal flexbox with:
  - Icon: `42px` font size
  - Title: `18px` font size, `font-weight: 600`
  - Subtitle: `12px` font size, monospace font, `color: #888`

**Example (Audio Player):**
```
┌────────────────────────────────────────────┐
│ 🎵  No audio file loaded                   │
│     Use File → Open File or Open Folder... │
└────────────────────────────────────────────┘
```

#### Content Area
- **Height**: Flexible (fills remaining space)
- **Background**: Varies by tool (typically `#1e1e1e` or `#252526`)
- **Overflow**: `auto` for scrollable content

### 4. Sidebar/Side Panel (Right)
- **Width**: `300px` (default, resizable in some tools)
- **Background**: `#252526`
- **Border**: `1px solid #3c3c3c` (left)
- **Features**:
  - File tree navigation
  - Queue management
  - Subtitle display
  - Context-specific content

### 5. Arrow Area (Web Analyzer Only)
- **Width**: `48px`
- **Background**: `#2b2b2b`
- **Borders**: `1px solid #1e1e1e` (left and right)
- **Button Size**: `32px × 32px`
- **Gap**: `20px` between buttons

## Audio Player Specific Example

### Layout Breakdown:
```
┌─────────────────────────────────────────────────────────────┐
│ Top Header: Settings Button Area (50px)                    │
├─────────────────────────────────────────────────────────────┤
│ Title Header (60px)                                         │
│ 🎵 [Icon]  Current Audio File.mp3                           │
│            C:\Path\To\File\Current Audio File.mp3           │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ Media Player Area (200px fixed)                             │
│           [Large Play Button]                               │
│                                                             │
├─────────────────────────────────────────────────────────────┤
│ Controls Bar (60px fixed)                                   │
│ [Progress Bar]                                              │
│ [⏹] [⏮] [▶] [⏭] [Volume] [Loop] [Time]                      │
├─────────────────────────────────────────────────────────────┤
│                                                             │
│ Metadata Section (Flexible - scrollable)                    │
│ 🎵 Audio Information                                        │
│   - Title, Artist, Album...                                 │
│                                                             │
└─────────────────────────────────────────────────────────────┘
```

## Color Palette

### Backgrounds
- **Primary**: `#1e1e1e` (main dark background)
- **Secondary**: `#2d2d30` (header areas)
- **Tertiary**: `#252526` (sidebar, panels)
- **Docker/Menu**: `#2b2b2b` and `#3c3c3c`

### Accents
- **Primary Accent**: `#4a9eff` (blue - active states, borders)
- **Hover**: `#007acc` (darker blue)
- **Success**: `#44ff44`
- **Error**: `#ff6b6b`

### Text
- **Primary**: `#cccccc`
- **Secondary**: `#888888`
- **Headers**: `#ffffff` or `#007acc`
- **Code**: `#ce9178`

### Borders
- **Primary**: `#3c3c3c`
- **Secondary**: `#1e1e1e`
- **Accent**: `#007acc`

## Window Configuration

### Application Window
- **Default State**: Maximized
- **Minimum Size**: Not specified (follows system defaults)
- **Decorations**: Custom (using Tauri decorations)
- **Drag Region**: Menu bar area

### Tauri Configuration
```json
{
  "windows": [{
    "fullscreen": false,
    "resizable": true,
    "maximized": true,
    "title": "FlexDesk"
  }]
}
```

## Flexbox Layout Pattern

### App Container
```css
.app {
  display: flex;
  flex-direction: column;
  height: 100vh;
  overflow: hidden;
}
```

### App Body (Horizontal Split)
```css
.app-body {
  display: flex;
  flex: 1;
  overflow: hidden;
}
```

### Component Order (Left to Right)
1. DockerBar (`48px` fixed)
2. MainArea (flexible, `flex: 1`)
3. ArrowArea (`48px` fixed, conditional)
4. Sidebar (`300px` default, may be resizable)

## Key Implementation Notes

### 1. Fixed vs Flexible Sizing
- **Fixed**: Menu bar, docker bar, headers, control bars
- **Flexible**: Main content areas, scrollable sections
- **Resizable**: Sidebar (in some tools)

### 2. Icon Consistency
- Docker bar icons: `24px`
- Header icons: `42px`
- Standard icons in content: `16-20px`

### 3. Header Pattern (All Tools Should Follow)
```
Top Header (optional) → Settings/Controls
Title Header (required) → Icon + Title + Subtitle
Content Area (required) → Tool-specific content
```

### 4. Spacing Standards
- Small gap: `8-12px`
- Medium gap: `15-20px`
- Large gap: `30px+`
- Section padding: `20px`
- Content padding: `15-20px`

### 5. Typography
- **Headers**: 16-18px, font-weight 600
- **Subheaders**: 14-16px, font-weight 500
- **Body**: 13-14px, normal weight
- **Small/Secondary**: 12-13px
- **Font Family**: System fonts (Segoe UI, SF Pro, etc.)

## Menu Structure

### File Menu
- Open File
- Open Folder
- Exit

### Edit Menu
- Settings

### View Menu
- Zoom In
- Zoom Out
- Reset Zoom

### Help Menu
- About

## Responsive Behavior

### On Window Resize
- Docker bar: Fixed width
- Main area: Grows/shrinks proportionally
- Sidebar: Maintains width (or resizable)
- Content: Scrollable if exceeds viewport

### Overflow Handling
- **Main Area**: `overflow: hidden` on container, `overflow: auto` on content sections
- **Sidebar**: `overflow-y: auto` for file trees and lists
- **Headers**: `overflow: hidden` with `text-overflow: ellipsis`

## Building This Layout

### Minimum Required Structure
```html
<div class="app">
  <MenuBar />
  <div class="app-body">
    <DockerBar />
    <MainArea>
      <TopHeader />      <!-- Optional -->
      <TitleHeader />    <!-- Required -->
      <ContentArea />    <!-- Required -->
    </MainArea>
    <Sidebar />
  </div>
</div>
```

### CSS Requirements
```css
* { margin: 0; padding: 0; box-sizing: border-box; }
html, body, #app { width: 100%; height: 100%; overflow: hidden; }
.app { display: flex; flex-direction: column; height: 100vh; }
.app-body { display: flex; flex: 1; overflow: hidden; }
```

## Tools/Views

Each tool follows this pattern with variations:
- **Home**: Welcome screen
- **Audio Player**: Media player with metadata
- **Video Player**: Similar to audio with video display
- **Document Review**: Text viewer with file tree
- **Quick Book**: Note-taking interface
- **Smart Downloader**: YouTube download manager
- **Misc Video Downloader**: Generic video downloader
- **Web Analyzer**: Web scraping tool with arrow controls
- **Learning Center**: Course management
- **AI Tools**: AI chat and tools interface

---

## Quick Book Implementation Details

Quick Book is a text/markdown editor with file tree navigation. It follows the standard FlexDesk layout pattern with specific behaviors for default folder loading.

### Layout Structure

```
┌─────────────────────────────────────────────────────────────┐
│ MenuBar (35px)                                              │
├──────┬────────────────────────────────────────┬─────────────┤
│      │ Top Header (50px)                      │  Sidebar    │
│Docker│ [📝 Quick Book]              [⚙️]      │  (300px)    │
│ Bar  ├────────────────────────────────────────┤             │
│(48px)│ Title Header (60px)                    │  ┌─────────┐│
│      │ filename.md                            │  │Files 🔄➕││
│      │ C:\path\to\file                        │  ├─────────┤│
│      ├────────────────────────────────────────┤  │TreeView ││
│      │                                        │  │  📁 dir ││
│      │ Editor Content Area                    │  │  📄 file││
│      │ (Textarea or Markdown Preview)         │  │  📄 file││
│      │                                        │  └─────────┘│
└──────┴────────────────────────────────────────┴─────────────┘
```

### 1. Top Header

**Location**: `QuickBookView.vue` → `.top-header`

**Dimensions**:
- **Height**: `~50px` (padding: 12px 20px)
- **Background**: `#252525`
- **Border**: `1px solid #333` (bottom)

**Layout**: Flexbox with `justify-content: space-between`

**Components**:
```
┌──────────────────────────────────────────────────────────────┐
│ [📝 Icon] [Quick Book Title]                      [⚙️ Btn] │
│ ← top-header-left                                 ← right  │
└──────────────────────────────────────────────────────────────┘
```

**Left Side** (`.top-header-left`):
- Icon: `📝` (font-size: 24px)
- Title: "Quick Book" (font-size: 16px, font-weight: 600, color: #ffffff)
- Gap: `12px`

**Right Side**:
- Settings Button (`.settings-btn`):
  - Content: `⚙️`
  - Background: `#3a3a3a`
  - Border: none
  - Padding: `8px 12px`
  - Border-radius: `4px`
  - Hover: Background `#4a9eff`, transform `scale(1.05)`
  - Click: Opens Settings Modal

**CSS**:
```css
.top-header {
  background-color: #252525;
  border-bottom: 1px solid #333;
  padding: 12px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-shrink: 0;
}
```

### 2. Settings Modal

**Trigger**: Click on `⚙️` button in top header

**Modal Structure**:
```
┌────────────────────────────────────────────────┐
│ Quick Book Settings                         × │
├────────────────────────────────────────────────┤
│ Configure default folder for Quick Book       │
│                                                │
│ Default Folder                                 │
│ ┌─────────────────────┬──────────┬──────────┐ │
│ │ C:\path\to\folder   │ 📁Browse │ 🗑️Clear │ │
│ └─────────────────────┴──────────┴──────────┘ │
│ Select a default folder to automatically      │
│ load when clicking Quick Book in docker bar   │
│                                                │
├────────────────────────────────────────────────┤
│                    [Cancel] [💾 Save Settings] │
└────────────────────────────────────────────────┘
```

**Components**:
- **Overlay**: `.dialog-overlay` - Dark backdrop with `rgba(0, 0, 0, 0.7)`
- **Dialog**: `.dialog` - Background `#2d2d30`, border-radius `8px`, max-width `500px`
- **Header**: Title + Close button (`×`)
- **Body**: Description + Folder input group
- **Footer**: Cancel + Save buttons
- **Save Message Banner**: Shows `✅ Settings saved!` on success

**Storage**: Uses `localStorage` with key `quickbook-default-folder`

### 3. Default Folder Behavior

**On Mount** (`loadSavedDefaultFolder`):
1. Reads `localStorage.getItem('quickbook-default-folder')`
2. If folder exists, calls `invoke('is_directory', { path: savedFolder })`
3. If valid directory, calls `loadFiles([savedFolder])`
4. This triggers sidebar tree view population

**Flow**:
```
Docker Bar Click → QuickBookView mounts
                 → loadSavedDefaultFolder()
                 → localStorage.getItem()
                 → is_directory check
                 → loadFiles([folder])
                 → emit('file-list-changed', fileTree, flatFiles, rootPath)
                 → SideBar receives tree data
                 → TreeView renders
```

### 4. Sidebar Implementation

**Location**: `SideBar.vue` → Tree View Mode section

**Activation Condition**:
```typescript
v-else-if="useTreeView && fileTree"
```

**Computed**: `useTreeView` returns `true` when `activeTool === 'quick-book'`

**Structure**:
```
┌─────────────────────────────────────────┐
│ Tree Header                             │
│ ┌─────────────────────────────────────┐ │
│ │ Files              [🔄] [➕]        │ │
│ └─────────────────────────────────────┘ │
├─────────────────────────────────────────┤
│ Tree View (scrollable)                  │
│ ┌─────────────────────────────────────┐ │
│ │ 📁 folder-name                      │ │
│ │   📄 file1.md                       │ │
│ │   📄 file2.txt                      │ │
│ │ 📁 another-folder                   │ │
│ │   📄 file3.hl7                      │ │
│ └─────────────────────────────────────┘ │
└─────────────────────────────────────────┘
```

#### Tree Header (`.tree-header`)

**Visibility**: Only for `quick-book` and `web-analyzer` tools

**Layout**:
```css
.tree-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 8px 12px;
  border-bottom: 1px solid #3a3a3a;
  background: #2d2d30;
}
```

**Components**:
- **Title** (`.tree-header-title`): "Files" - font-size: 12px, color: #888
- **Actions** (`.tree-header-actions`): Flexbox with gap: 4px
  - **Refresh Button** (`🔄`): Calls `refreshTreeView()`
  - **Add Button** (`➕`): Calls `showNewFileDialog()`

**Button Styles** (`.tree-action-btn`):
```css
.tree-action-btn {
  background: transparent;
  border: none;
  color: #888;
  cursor: pointer;
  padding: 4px 6px;
  border-radius: 4px;
  font-size: 14px;
}
.tree-action-btn:hover {
  background: #3a3a3a;
  color: #fff;
}
```

#### Tree View Area (`.tree-view`)

**Component**: Uses `FileTreeNode` recursive component

**Props passed to FileTreeNode**:
- `node`: FileNode object with `name`, `path`, `is_dir`, `children`
- `active-path`: Currently selected file path (for highlighting)
- `@file-selected`: Emits path when file is clicked

**Placeholder**: Shows "No files found in tree" when empty

### 5. FileTreeNode Component

**Location**: `components/FileTreeNode.vue`

**Features**:
- Recursive rendering for nested folders
- Folder expand/collapse with `▼` / `▶` icons
- File icons: `📁` for folders, `📄` for files
- Active state highlighting (background: #094771)
- Hover effect (background: #2a2d2e)

**Events**:
- `@file-selected`: Bubbles up file path when clicked
- Opens file in main editor area

### 6. File Types Supported

Quick Book filters for text-based files:
```typescript
const textExtensions = ['txt', 'md', 'hl7'];
```

### 7. Communication Flow

```
┌─────────────────────────────────────────────────────────────┐
│                     QuickBookView.vue                        │
│  ┌─────────────┐                    ┌─────────────────────┐ │
│  │ Top Header  │                    │ Editor Area         │ │
│  │ [Settings]  │                    │ (textarea/preview)  │ │
│  └──────┬──────┘                    └──────────▲──────────┘ │
│         │                                      │             │
│         │ emit('file-list-changed')           │ loadTextFile│
│         ▼                                      │             │
├─────────────────────────────────────────────────────────────┤
│                       SideBar.vue                            │
│  ┌─────────────────────────────────────────────────────────┐│
│  │ Tree Header: [Files] [🔄] [➕]                          ││
│  ├─────────────────────────────────────────────────────────┤│
│  │ FileTreeNode (recursive)                                ││
│  │   - Click folder → expand/collapse                      ││
│  │   - Click file → emit('file-selected', path)            ││
│  └─────────────────────────────────────────────────────────┘│
│                            │                                 │
│                            │ @file-selected                  │
│                            ▼                                 │
│                  QuickBookView.loadTextFile(path)            │
└─────────────────────────────────────────────────────────────┘
```

### 8. CSS Class Reference

| Class | Component | Purpose |
|-------|-----------|---------|
| `.quick-book-view` | Main container | Flex column, full height |
| `.top-header` | Header bar | Contains icon, title, settings |
| `.top-header-left` | Left section | Icon + title grouping |
| `.settings-btn` | Settings button | Opens modal |
| `.dialog-overlay` | Modal backdrop | Dark overlay |
| `.dialog` | Modal container | Settings form |
| `.tree-view-container` | Sidebar section | Contains tree header + view |
| `.tree-header` | Tree toolbar | Title + action buttons |
| `.tree-action-btn` | Toolbar buttons | Refresh, Add |
| `.tree-view` | Scrollable area | FileTreeNode list |
| `.editor-container` | Main editor | When file is loaded |
| `.no-file-container` | Empty state | When no file selected |

---

**Last Updated**: January 2026  
**Version**: 1.1  
**Framework**: Tauri 2.x + Vue 3 + TypeScript