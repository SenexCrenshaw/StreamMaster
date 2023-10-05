import { VideoStreamDto } from '@/lib/iptvApi'
import { useDispatch, useSelector } from 'react-redux'
import { AppDispatch, RootState } from '../store'
import {
  makeSelectedVideoStreams,
  setSelectedVideoStreamsInternal,
} from './selectedVideoStreamsSlice'

export const useSelectedVideoStreams = (typename: string) => {
  const dispatch: AppDispatch = useDispatch()

  // Use the memoized selector
  const selectedVideoStreams = useSelector((state: RootState) =>
    makeSelectedVideoStreams(state, typename),
  )

  const setSelectedVideoStreams = (videoStreamDtos: VideoStreamDto[]) => {
    dispatch(
      setSelectedVideoStreamsInternal({
        typename,
        videoStreamDtos,
      }),
    )
  }

  return { selectedVideoStreams, setSelectedVideoStreams }
}
