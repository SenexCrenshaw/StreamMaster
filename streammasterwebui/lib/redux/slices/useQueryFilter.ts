import { useDispatch, useSelector } from 'react-redux';

import { GetApiArgument } from '@lib/apiDefs';
import { type AppDispatch, type RootState } from '../store';
import { setQueryFilterInternal } from './queryFilterSlice';

export const useQueryFilter = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setQueryFilter = (filter: GetApiArgument) => {
    dispatch(
      setQueryFilterInternal({
        filter,
        typename
      })
    );
  };

  const queryFilter = useSelector((rootState: RootState) => rootState.queryFilter[typename]);

  return { queryFilter, setQueryFilter };
};
