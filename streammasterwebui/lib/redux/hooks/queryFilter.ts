import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';
import { QueryStringParameters } from '@lib/smAPI/smapiTypes';

interface SetQueryFilterPayload {
  value: QueryStringParameters | undefined;
  key: string;
}

type QueryFilterState = Record<string, QueryStringParameters | undefined>;

const initialState: QueryFilterState = {};

const queryFilterSlice = createSlice({
  initialState,
  name: 'queryFilter',
  reducers: {
    SetQueryFilter: (state, action: PayloadAction<SetQueryFilterPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectQueryFilter = createSelector(
  (state: RootState) => state.queryFilter,
  (queryFilter) => (key: string) => queryFilter[key]
);

export const useQueryFilter = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setQueryFilter = (value: QueryStringParameters | undefined) => {
    dispatch(
      queryFilterSlice.actions.SetQueryFilter({
        key,
        value
      })
    );
  };

  const queryFilter = useSelector((state: RootState) => selectQueryFilter(state)(key));

  return { queryFilter, setQueryFilter };
};

export default queryFilterSlice.reducer;
