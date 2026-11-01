document.addEventListener('DOMContentLoaded', function () {
    const audio = document.getElementById('audio-player');
    const playerTitle = document.getElementById('player-title');
    const playerArtist = document.getElementById('player-artist');
    const playerImg = document.getElementById('player-img');
    const mainPlayBtn = document.getElementById('main-play-btn');
    const progressBar = document.getElementById('progress-bar');
    const currentTimeEl = document.getElementById('current-time');
    const durationEl = document.getElementById('duration');
    const volumeBar = document.getElementById('volume-bar');

    if (!audio) return;

    let isPlaying = false;

    // Hàm toàn cục - được gọi từ HTML onclick
    window.playSong = function (filePath, title, artist, image) {
        const src = '/uploads/' + filePath;

        if (audio.src !== location.origin + src) {
            audio.src = src;
            playerTitle.textContent = title;
            playerArtist.textContent = artist;
            playerImg.src = '/uploads/' + (image || 'default.jpg');
        }

        if (isPlaying) {
            audio.pause();
        } else {
            audio.play();
        }
    };

    // Nút play/pause chính
    if (mainPlayBtn) {
        mainPlayBtn.addEventListener('click', function () {
            if (!audio.src) return;
            if (audio.paused) audio.play();
            else audio.pause();
        });
    }

    audio.addEventListener('play', () => {
        isPlaying = true;
        mainPlayBtn.innerHTML = '<i class="fas fa-pause"></i>';
    });

    audio.addEventListener('pause', () => {
        isPlaying = false;
        mainPlayBtn.innerHTML = '<i class="fas fa-play"></i>';
    });

    audio.addEventListener('timeupdate', () => {
        const percent = (audio.currentTime / audio.duration) * 100 || 0;
        progressBar.value = percent;
        currentTimeEl.textContent = formatTime(audio.currentTime);
    });

    audio.addEventListener('loadedmetadata', () => {
        durationEl.textContent = formatTime(audio.duration);
    });

    progressBar.addEventListener('input', () => {
        audio.currentTime = (progressBar.value / 100) * audio.duration;
    });

    volumeBar.addEventListener('input', () => {
        audio.volume = volumeBar.value / 100;
    });

    audio.volume = 0.8;

    function formatTime(sec) {
        if (isNaN(sec)) return '0:00';
        const m = Math.floor(sec / 60);
        const s = Math.floor(sec % 60).toString().padStart(2, '0');
        return `${m}:${s}`;
    }
});