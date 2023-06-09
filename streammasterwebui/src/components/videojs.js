import React from 'react';
import videojs from 'video.js';
import Hls from "hls.js";

import 'video.js/dist/video-js.css';

export const VideoJS = (props) => {
  const videoRef = React.useRef(null);
  const playerRef = React.useRef(null);
  const {options, onReady} = props;



  React.useEffect(() => {

    // Make sure Video.js player is only initialized once
    if (!playerRef.current) {
      // The Video.js player needs to be _inside_ the component el for React 18 Strict Mode.
      const videoElement = document.createElement("video-js");

      videoElement.classList.add('vjs-big-play-centered');
      videoRef.current.appendChild(videoElement);



      const player = playerRef.current = videojs(videoElement, options, () => {
        videojs.log.level('all')
        videojs.log('player is ready');
        player.tech().on('usage', (e) => {
          videojs.log(e.name);
        });
        onReady && onReady(player);
      });

      if (Hls.isSupported()) {
          videojs.log('HLS Supported');
        var hls = new Hls();

        hls.loadSource('http://192.168.1.216:7095/api/streamgroups/1/stream/10852.ts');
        hls.attachMedia(player);

        hls.on(Hls.Events.ERROR,function() {
          videojs.log('HLS ERROR');
        });

        hls.on(Hls.Events.MANIFEST_PARSED,function() {
          player.play();
      });
      }else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        player.src = 'https://video-dev.github.io/streams/x36xhzz/x36xhzz.m3u8';
        player.addEventListener('canplay',function() {
          player.play();
    });
  }

    // You could update an existing player in the `else` block here
    // on prop change, for example:
    } else {
      const player = playerRef.current;

    //      const hls = new Hls();
    // const url = "https://bitdash-a.akamaihd.net/content/sintel/hls/playlist.m3u8";

    // hls.loadSource(url);
    // hls.attachMedia(player);
    //   hls.on(Hls.Events.MANIFEST_PARSED, function () { player.play(); });

      // player.autoplay(options.autoplay);
      // player.src(options.sources);
    }
  }, [options, videoRef]);

  // Dispose the Video.js player when the functional component unmounts
  React.useEffect(() => {
    const player = playerRef.current;

    return () => {
      if (player && !player.isDisposed()) {
        player.dispose();
        playerRef.current = null;
      }
    };
  }, [playerRef]);

  return (
    <div data-vjs-player>
      <div ref={videoRef} />
    </div>
  );
}

export default VideoJS;
