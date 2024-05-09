import { GetApiArgument } from '@lib/apiDefs';
import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetQueryFilterPayload {
  value: GetApiArgument | undefined;
  key: string;
}

type QueryFilterState = Record<string, GetApiArgument | undefined>;

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

  const setQueryFilter = (value: GetApiArgument | undefined) => {
    dispatch(
      queryFilterSlice.actions.SetQueryFilter({
        key,
        value
      })
    );
  };

  const queryFilter = useSelector((state: RootState) => selectQueryFilter(state)(key));

  return { setQueryFilter, queryFilter };
};

export default queryFilterSlice.reducer;
