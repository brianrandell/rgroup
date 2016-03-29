function makeImageUploadTarget(target, dropZone, progressBar, onUploadDone) {
    'use strict';

    target.fileupload({
        url: '/api/UploadMedia',
        dropZone: dropZone,
        acceptFileTypes: /(\.|\/)(gif|jpe?g|png)$/i,
        maxFileSize: 999000,
        maxNumberOfFiles: 1,
    }).on('fileuploadprocessalways', function (e, data) {
        var index = data.index,
            file = data.files[index],
            node = $(data.context.children()[index]);
        if (file.preview) {
            node
                .prepend('<br>')
                .prepend(file.preview);
        }
        if (file.error) {
            node
                .append('<br>')
                .append($('<span class="text-danger" />').text(file.error));
        }
    }).on('fileuploadadd', function (e, data) {
        data.context = $('<div />').appendTo('#files');
        $.each(data.files, function (index, file) {
            var node = $('<p />')
                    .append($('<span />').text(file.name));
            node.appendTo(data.context);
        });
    }).on('fileuploadprogressall', function (e, data) {
        var progress = parseInt(data.loaded / data.total * 100, 10);
        $('#progress .progress-bar').css(
            'width',
            progress + '%'
        );
    }).on('fileuploaddone', onUploadDone);
}
