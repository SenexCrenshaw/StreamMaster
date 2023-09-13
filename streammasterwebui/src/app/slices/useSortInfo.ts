import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';
import { setSortInfoInternal } from './sortInfoSlice';

export const useSortInfo = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSortInfo = (isSortInfo: { sortField?: string, sortOrder?: -1 | 0 | 1, }) => {
    dispatch(setSortInfoInternal({
      sortField: isSortInfo.sortField,
      sortOrder: isSortInfo.sortOrder,
      typename,
    }));
  };

  const sortInfo = useSelector((rootState: RootState) => rootState.sortInfo[typename]);

  return { setSortInfo, sortInfo };
};
