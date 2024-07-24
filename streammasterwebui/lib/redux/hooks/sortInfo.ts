import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

export interface SortInfo {
  orderBy?: string;
  sortField?: string;
  sortOrder?: -1 | 0 | 1;
  typename: string;
}

type SortInfoState = Record<string, { orderBy: string; sortField: string; sortOrder: -1 | 0 | 1 }>;

const defaultSortInfo = {
  orderBy: 'Id asc',
  sortField: 'Id',
  sortOrder: 1 as -1 | 0 | 1
};
const initialState: SortInfoState = {};

const sortInfoSlice = createSlice({
  initialState,
  name: 'sortInfo',
  reducers: {
    setSortInfo: (state, action: PayloadAction<SortInfo>) => {
      const { typename, sortField, sortOrder } = action.payload;

      if (!state[typename]) {
        state[typename] = { ...defaultSortInfo };
      }

      if (sortField !== undefined) {
        state[typename].sortField = sortField;
      }

      if (sortOrder !== undefined) {
        state[typename].sortOrder = sortOrder;
      }

      if (state[typename].sortField && state[typename].sortOrder) {
        const newValue = state[typename].sortField
          ? state[typename].sortOrder === -1
            ? `${state[typename].sortField} desc`
            : `${state[typename].sortField} asc`
          : '';

        state[typename].orderBy = newValue;
      }
    }
  }
});

export const useSortInfo = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSortInfo = (isSortInfo: { sortField?: string; sortOrder?: -1 | 0 | 1 }) => {
    const newValue = isSortInfo.sortField ? (isSortInfo.sortOrder === -1 ? `${isSortInfo.sortField} desc` : `${isSortInfo.sortField} asc`) : '';

    dispatch((action) => {
      action(
        sortInfoSlice.actions.setSortInfo({
          orderBy: newValue,
          sortField: isSortInfo.sortField,
          sortOrder: isSortInfo.sortOrder,
          typename
        })
      );
    });
  };

  const sortInfo = useSelector((rootState: RootState) => rootState.sortInfo[typename]);

  return { setSortInfo, sortInfo };
};
export default sortInfoSlice.reducer;
