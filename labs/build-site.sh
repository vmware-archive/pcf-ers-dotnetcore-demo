
#!/bin/bash

echo cleaning site...
rm -R site

echo Generating main page...
asciidoctor "*.adoc" -D site

echo Generating labs...
asciidoctor "**/lab*.adoc" -D site

echo Generating slides...
asciidoctor -T asciidoctor-deck.j/templates/haml/ "**/slides*.adoc" -D site

echo Moving deck.js...
cp -r deck.js ./site/deck.js
