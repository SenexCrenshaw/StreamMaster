import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../../../lib/redux/store';
import { type AdditionalFilterProps } from '../../common/common';
import { setQueryAdditionalFilter as setQueryAdditionalFilterInternal } from './queryAdditionalFiltersSlice';


export const useQueryAdditionalFilters = (typename: string) => {

  const dispatch: AppDispatch = useDispatch();

  const setQueryAdditionalFilter = (newFilter: AdditionalFilterProps) => {
    dispatch(setQueryAdditionalFilterInternal({
      filter: newFilter,
      typename,
    }));
  };

  const queryAdditionalFilter = useSelector((rootState: RootState) => rootState.queryAdditionalFilters[typename]);

  return { queryAdditionalFilter, setQueryAdditionalFilter };
};

