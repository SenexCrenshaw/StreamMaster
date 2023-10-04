import { useDispatch, useSelector } from 'react-redux'
import { type AppDispatch, type RootState } from '../../../lib/redux/store'
import { setSelectedItemsInternal } from './selectedItemsSlice'

const castToArrayOfType = <T>(data: any): T[] => {
  if (data === undefined) {
    return [] as T[]
  }

  if (Array.isArray(data)) {
    return data as T[]
  }

  throw new Error('Data is not of the expected type.', data)
}

export const useSelectedItems = <T>(typename: string) => {
  const dispatch: AppDispatch = useDispatch()

  const setSelectSelectedItems = (items: T[]) => {
    dispatch(
      setSelectedItemsInternal({
        items: items,
        typename,
      }),
    )
  }

  const rawSelectedItems = useSelector(
    (rootState: RootState) => rootState.selectedItems[typename],
  )
  const selectSelectedItems = castToArrayOfType<T>(rawSelectedItems)
  return { selectSelectedItems, setSelectSelectedItems }
}
