import { useDispatch, useSelector } from 'react-redux';
import { type AdditionalFilterProperties } from '../../common/common';
import { type AppDispatch, type RootState } from '../store';
import { setQueryAdditionalFilter as setQueryAdditionalFilterInternal } from './queryAdditionalFiltersSlice';

export const useQueryAdditionalFilters = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setQueryAdditionalFilter = (newFilter: AdditionalFilterProperties) => {
    dispatch(
      setQueryAdditionalFilterInternal({
        filter: newFilter,
        typename
      })
    );
  };

  const queryAdditionalFilter = useSelector((rootState: RootState) => rootState.queryAdditionalFilters[typename]);

  return { queryAdditionalFilter, setQueryAdditionalFilter };
};
