import { useDispatch, useSelector } from 'react-redux';
import { type VideoStreamDto } from '../../store/iptvApi';
import { type AppDispatch, type RootState } from '../store';
import { makeSelectedVideoStreams, setSelectedVideoStreamsInternal } from './selectedVideoStreamsSlice';

export const useSelectedVideoStreams = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  // Use the memoized selector
  const selectedVideoStreams = useSelector((state: RootState) => makeSelectedVideoStreams(state, typename));

  const setSelectedVideoStreams = (videoStreamDtos: VideoStreamDto[]) => {
    dispatch(setSelectedVideoStreamsInternal({
      typename,
      videoStreamDtos,
    }));
  };

  return { selectedVideoStreams, setSelectedVideoStreams };
};

