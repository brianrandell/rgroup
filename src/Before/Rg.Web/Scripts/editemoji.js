var editAutoCompleteEmojiSettings = {
    match: /\B:([\-+\w]*)$/,
    search: function (term, callback) {
        var results = [];
        var results2 = [];
        var results3 = [];
        $.each(emojiStrategy, function (shortname, data) {
            if (shortname.indexOf(term) > -1) { results.push(shortname); }
            else {
                if ((data.aliases !== null) && (data.aliases.indexOf(term) > -1)) {
                    results2.push(shortname);
                }
                else if ((data.keywords !== null) && (data.keywords.indexOf(term) > -1)) {
                    results3.push(shortname);
                }
            }
        });

        if (term.length >= 3) {
            results.sort(function (a, b) { return (a.length > b.length); });
            results2.sort(function (a, b) { return (a.length > b.length); });
            results3.sort();
        }
        var newResults = results.concat(results2).concat(results3);

        callback(newResults);
    },
    template: function (shortname) {
        return '<img class="emojione" src="//cdn.jsdelivr.net/emojione/assets/png/' + emojiStrategy[shortname].unicode + '.png"> :' + shortname + ':';
    },
    replace: function (shortname) {
        return ':' + shortname + ': ';
    },
    index: 1,
    maxCount: 10
};

var editAutoCompleteUserSettings = {
    match: /\B@([\-+\w]*)$/,
    search: function (term, callback) {
        var results = [];
        $.each(userCompletionList, function (email, entry) {
            var name = entry.Name;
            if (name.indexOf(term) > -1 || email.indexOf(term) > -1) { results.push(email); }
        });
        callback(results);
    },
    template: function (email) {
        var entry = userCompletionList[email];
        return '<img class="usersuggest avatar" src="' + entry.AvatarUrl + '"> @' + entry.Name;
    },
    replace: function (email) {
        return '@' + email + '@ ';
    },
    index: 1,
    maxCount: 10
};

function enableEditAutoCompletion(textArea) {
    textArea.textcomplete([editAutoCompleteEmojiSettings, editAutoCompleteUserSettings]);
}