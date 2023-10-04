import { useDispatch, useSelector } from 'react-redux'
import { type AppDispatch, type RootState } from '../../../lib/redux/store'
import { setChannelGroupToRemove as setChannelGroupToRemoveInternal } from './channelGroupToRemoveSlice'

export const useChannelGroupToRemove = (typename: string) => {
  const dispatch: AppDispatch = useDispatch()

  const setChannelGroupToRemove = (toRemove: number) => {
    dispatch(
      setChannelGroupToRemoveInternal({
        toRemove: toRemove,
        typename,
      }),
    )
  }

  const channelGroupToRemove = useSelector(
    (rootState: RootState) => rootState.channelGroupToRemove[typename],
  )

  return { channelGroupToRemove, setChannelGroupToRemove }
}
