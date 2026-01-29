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

**Last Updated**: January 2026  
**Version**: 1.0  
**Framework**: Tauri 2.x + Vue 3 + TypeScript