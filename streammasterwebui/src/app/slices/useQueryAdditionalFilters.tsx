import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';
import { setQueryAdditionalFilter as setQueryAdditionalFilterInternal } from './queryAdditionalFiltersSlice';
import { type AdditionalFilterProps } from '../../common/common';


export const useQueryAdditionalFilters = (typename: string) => {

  const dispatch: AppDispatch = useDispatch();

  const setQueryAdditionalFilter = (newFilter: AdditionalFilterProps) => {
    dispatch(setQueryAdditionalFilterInternal({
      filter: newFilter,
      typename
    }));
  };

  const queryAdditionalFilter = useSelector((rootState: RootState) => rootState.queryAdditionalFilters[typename]);

  return { queryAdditionalFilter, setQueryAdditionalFilter };
};

