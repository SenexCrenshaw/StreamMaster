import { useDispatch, useSelector } from 'react-redux'
import { type AppDispatch, type RootState } from '../../../lib/redux/store'
import {
  setHubDisconnected as setDisconnectedInternal,
  setHubConnected as setHubConnectedInternal,
} from './appInfoSlice'

export const useAppInfo = () => {
  const dispatch: AppDispatch = useDispatch()

  const setHubDisconnected = (isDisconnectedValue: boolean) => {
    dispatch(setDisconnectedInternal(isDisconnectedValue))
  }

  const setHubConnected = (isHubConnectedValue: boolean) => {
    dispatch(setHubConnectedInternal(isHubConnectedValue))
  }

  const isHubConnected = useSelector(
    (rootState: RootState) => rootState.appInfo.isHubConnected,
  )

  return { isHubConnected, setHubConnected, setHubDisconnected }
}
