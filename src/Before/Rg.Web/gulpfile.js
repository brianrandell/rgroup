/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var uglify = require('gulp-uglify');
var concat = require('gulp-concat');

gulp.task('default', function () {
    // place code for your default task here
});

gulp.task('stylecontent', function () {
    gulp.src([
        'lib/bootstrap/dist/css/bootstrap.min.css'
    ])
    .pipe(gulp.dest('Content'));
    gulp.src([
        'lib/bootstrap/fonts/**'
    ])
    .pipe(gulp.dest('fonts'));
});

//gulp.task('script-editemoji', function () {
//    gulp.src([
//        'lib/jquery-ui/jquery-ui.js',
//        'lib/blueimp-load-image/js/load-image.all.min.js',
//        'lib/blueimp-canvas-to-blob/js/canvas-to-blob.js',
//        'lib/jquery-textcomplete/dist/jquery.textcomplete.js',
//        'Scripts/emoji-strategy.js',
//        'Scripts/editemoji.js'])
//        .pipe(uglify())
//        .pipe(concat('editemoji.all.min.js'))
//        .on('error', function (err) {
//            console.error('Error!', err.message);
//        })
//        .pipe(gulp.dest('Scripts'));

//});

gulp.task('script-upload', function () {
    gulp.src([
        'lib/jquery.iframe-transport/jquery.iframe-transport.js',
        'lib/jquery-ui/jquery-ui.js',
        'lib/blueimp-load-image/js/load-image.all.min.js',
        'lib/blueimp-canvas-to-blob/js/canvas-to-blob.js',
        'lib/blueimp-file-upload/js/jquery.fileupload.js',
        'lib/blueimp-file-upload/js/jquery.fileupload-process.js',
        'lib/blueimp-file-upload/js/jquery.fileupload-image.js',
        //'lib/blueimp-file-upload/js/jquery.fileupload-video.js',
        'lib/blueimp-file-upload/js/jquery.fileupload-validate.js',
        'Scripts/uploads.js'])
        .pipe(uglify())
        .pipe(concat('upload.min.js'))
        .on('error', function (err) {
            console.error('Error!', err.message);
        })
        .pipe(gulp.dest('Scripts'));

});

gulp.task('script-common', function () {
    gulp.src([
        'lib/jquery/dist/jquery.min.js',
        'lib/lib/bootstrap/dist/js/bootstrap.min.js',
        'lib/lib/respond/dest/respond.min.js',
        'lib/blueimp-tmpl/js/tmpl.js',
        'Scripts/showemoji.js',
        'lib/jquery-textcomplete/dist/jquery.textcomplete.js',
        'Scripts/emoji-strategy.js',
        'Scripts/editemoji.js',
        'Scripts/comments.js',
        'Scripts/likes.js',
        'Scripts/design.js'])
        .pipe(uglify())
        .pipe(concat('common.min.js'))
        .on('error', function (err) {
            console.error('Error!', err.message);
        })
        .pipe(gulp.dest('Scripts'));

    gulp.src([
        'lib/modernizr/modernizr.js'
    ])
    .pipe(gulp.dest('Scripts'));
});