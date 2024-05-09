import { AdditionalFilterProperties } from '@lib/common/common';
import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetQueryAdditionalFiltersPayload {
  value: AdditionalFilterProperties | null | undefined;
  key: string;
}

type QueryAdditionalFiltersState = Record<string, AdditionalFilterProperties | null | undefined>;

const initialState: QueryAdditionalFiltersState = {};

const queryAdditionalFiltersSlice = createSlice({
  initialState,
  name: 'queryAdditionalFilters',
  reducers: {
    SetQueryAdditionalFilters: (state, action: PayloadAction<SetQueryAdditionalFiltersPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectQueryAdditionalFilters = createSelector(
  (state: RootState) => state.queryAdditionalFilters,
  (queryAdditionalFilters) => (key: string) => queryAdditionalFilters[key]
);

export const useQueryAdditionalFilters = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setQueryAdditionalFilters = (value: AdditionalFilterProperties | null | undefined) => {
    dispatch(
      queryAdditionalFiltersSlice.actions.SetQueryAdditionalFilters({
        key,
        value
      })
    );
  };

  const queryAdditionalFilters = useSelector((state: RootState) => selectQueryAdditionalFilters(state)(key));

  return { queryAdditionalFilters, setQueryAdditionalFilters };
};

export default queryAdditionalFiltersSlice.reducer;
