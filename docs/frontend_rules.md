# Frontend Rules & Design System

## 1. Design Philosophy: "Modern & Premium"
- **Style**: Minimalist, clean lines, high contrast.
- **Reference**: Coolmate / Uniqlo / Ssense.
- **Spacing**: Generous whitespace. Use multiples of `4` (Tailwind standard).

## 2. Typography
- **Primary Font**: `Manrope` (Headings, Buttons) - Geometric, Modern.
- **Secondary Font**: `Inter` or `Plus Jakarta Sans` (Body text) - Readable.
- **Weights**:
  - Headings: `SemiBold` (600) or `Bold` (700).
  - Body: `Regular` (400) or `Medium` (500).

### Font Setup (Google Fonts)
```html
<link href="https://fonts.googleapis.com/css2?family=Manrope:wght@400;500;600;700;800&family=Inter:wght@300;400;500;600&display=swap" rel="stylesheet">
```

## 3. Color Palette
- **Neutral (Primary)**:
  - `Black`: `#111111` (Text, Strong Borders)
  - `Dark Gray`: `#4B5563` (Secondary Text)
  - `Light Gray`: `#F3F4F6` (Backgrounds)
  - `White`: `#FFFFFF`
- **Accent**:
  - `Electric Blue`: `#2563EB` (Primary Actions) - *Use sparingly.*
  - `Success`: `#10B981`
  - `Error`: `#EF4444`

## 4. Iconography
- **Library**: `Lucide Vue` (Standard, stroke-based, clean).
- **Rules**:
  - Stroke width: `1.5` or `2px` (Consistent).
  - Size: `20px` (sm), `24px` (md).
- **Usage**:
  ```ts
  import { ShoppingBag, Search, User } from 'lucide-vue-next';
  ```

## 5. Components Standards
- **Buttons**:
  - *Primary*: Black background, White text, Rounded-Full (Pill shape) or Rounded-lg.
  - *Secondary*: White background, Black border.
  - *Animation*: Scale 0.98 on click.
- **Inputs**:
  - No default borders. Bottom border or Gray background (`bg-gray-100`) with Focus Ring.
- **Cards**:
  - Flat design. No shadow by default. Shadow-lg on Hover.

## 6. Directory Structure
```
src/
  assets/        
  components/    
    ui/          # Generic UI atoms (Button, Input) - derived from Design System
  features/      # Domain logic
  layouts/       
  router/        
```
