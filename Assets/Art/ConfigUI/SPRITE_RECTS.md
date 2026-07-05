# Config UI Sprite Rects

`config_ui_spritesheet.png` is a transparent PNG. In Unity, set:

- Texture Type: Sprite (2D and UI)
- Sprite Mode: Multiple
- Mesh Type: Full Rect
- Filter Mode: Bilinear

Slice the sheet with these rects:

| Name | X | Y | W | H |
| --- | ---: | ---: | ---: | ---: |
| slider_track_full | 16 | 16 | 640 | 48 |
| slider_track_empty | 16 | 80 | 640 | 48 |
| slider_fill_blue | 16 | 144 | 512 | 24 |
| slider_handle_diamond | 560 | 128 | 64 | 64 |
| button_skew_black | 16 | 200 | 420 | 96 |
| category_highlight_black | 456 | 200 | 560 | 76 |
| arrow_left | 720 | 16 | 64 | 64 |
| arrow_right | 792 | 16 | 64 | 64 |
| toggle_on | 720 | 88 | 220 | 72 |
| toggle_off | 720 | 176 | 220 | 72 |
| star_marker_black | 872 | 16 | 64 | 64 |
| section_divider | 16 | 320 | 900 | 24 |

The same sprites are also exported as individual PNG files in this folder for quick UI setup.
