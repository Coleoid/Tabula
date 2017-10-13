use v6;

role Match-Helper {

    method line-of-match-start($match) {
        1 + ($match.prematch.comb: /\n/)
    }

    method lines-in-match($match) {
        1 + ($match.comb: /\n/)
    }

    method source-location($match) {
        'no file' ~ ':' ~ self.line-of-match-start($match);
        #(self.file-name // 'no file') ~ ':' ~ self.line-of-match-start($match);
    }

    method name-section($section-shape, $/) {
        my $start-line = self.line-of-match-start($/);
        my $end-line = $start-line + self.lines-in-match($/) - 2;

        sprintf("%s_lines_%03d_to_%03d", $section-shape, $start-line, $end-line);
    }

}
