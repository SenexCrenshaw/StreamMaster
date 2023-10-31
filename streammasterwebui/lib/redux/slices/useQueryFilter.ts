import { useDispatch, useSelector } from 'react-redux';
import { type GetApiArgument } from '../../common/common';
import { type AppDispatch, type RootState } from '../store';
import { setQueryFilterInternal } from './queryFilterSlice';

export const useQueryFilter = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setQueryFilter = (newFilter: GetApiArgument) => {
    dispatch(
      setQueryFilterInternal({
        filter: newFilter,
        typename
      })
    );
  };

  const queryFilter = useSelector((rootState: RootState) => rootState.queryFilter[typename]);

  return { queryFilter, setQueryFilter };
};
