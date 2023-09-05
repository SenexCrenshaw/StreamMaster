import { type PayloadAction } from '@reduxjs/toolkit';
import { createSlice } from '@reduxjs/toolkit';
import { type GetApiArg } from '../../common/common';
import { type RootState } from '../store';

type SetQueryFilterPayload = {
  filter: GetApiArg | undefined;
  typename: string;
}

type QueryFilterState = Record<string, GetApiArg | undefined>;

const initialState: QueryFilterState = {};

const queryFilterSlice = createSlice({
  initialState,
  name: 'queryFilter',
  reducers: {
    setQueryFilterInternal: (state, action: PayloadAction<SetQueryFilterPayload>) => {
      const { typename, filter } = action.payload;

      if (filter !== null && filter !== undefined) {
        state[typename] = filter;
      } else {

        delete state[typename]; // Remove the key if the filter is null or undefined
      }
    }
  }
});

export const selectQueryFilter = (state: RootState, typename: string) => state.queryFilter[typename];
export const { setQueryFilterInternal } = queryFilterSlice.actions;
export default queryFilterSlice.reducer;
