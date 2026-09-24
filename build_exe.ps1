$ErrorActionPreference = "Stop"

Write-Host "TTwen EXE build started..."

if (-not (Test-Path ".venv")) {
    py -m venv .venv
}

& ".\.venv\Scripts\python.exe" -m pip install --upgrade pip
& ".\.venv\Scripts\python.exe" -m pip install -r requirements.txt
& ".\.venv\Scripts\playwright.exe" install chromium
& ".\.venv\Scripts\pyinstaller.exe" --noconfirm --clean --onefile --windowed --name TTwen main.py

Write-Host ""
Write-Host "Build completed: dist\TTwen.exe"
