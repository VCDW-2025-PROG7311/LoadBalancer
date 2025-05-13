1..100 | ForEach-Object {
    $res = Invoke-WebRequest -Uri http://localhost:8090 -UseBasicParsing
    Write-Output "$($_) - $($res.Content)"
}

# You can explore rate limiting on your own.