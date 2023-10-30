import { createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { RootState } from '../store';

interface SetSortInfoPayload {
  sortField?: string;
  sortOrder?: -1 | 0 | 1;
  typename: string;
}

type SortInfoState = Record<string, { orderBy: string; sortField: string; sortOrder: -1 | 0 | 1 }>;

const defaultSortInfo = {
  orderBy: 'id asc',
  sortField: 'id',
  sortOrder: 1 as -1 | 0 | 1
};
const initialState: SortInfoState = {};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function updateOrderBy(existingOrderBy: string, orderBy: string): string {
  // Split the existingOrderBy string into an array of field-order pairs
  const orderByArray = existingOrderBy.split(',').map((entry) => entry.trim());

  // Split the orderBy string into field and order
  const [newField, newOrder] = orderBy.split(' ');

  // Find if the field already exists in the existingOrderBy array
  const fieldExists = orderByArray.some((entry) => {
    const [field] = entry.split(' ');
    return field === newField;
  });

  if (fieldExists) {
    // If the field already exists, update the order
    const updatedOrderByArray = orderByArray.map((entry) => {
      const [field] = entry.split(' ');
      if (field === newField) {
        return `${newField} ${newOrder}`;
      }
      return entry;
    });
    return updatedOrderByArray.join(', ');
  }
  // If the field doesn't exist, add it to the existingOrderBy string
  if (existingOrderBy) {
    return `${existingOrderBy}, ${orderBy}`;
  }
  return orderBy;
}

const sortInfoSlice = createSlice({
  initialState,
  name: 'sortInfo',
  reducers: {
    setSortInfoInternal: (state, action: PayloadAction<SetSortInfoPayload>) => {
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
          ? (state[typename].sortOrder === -1
            ? `${state[typename].sortField} desc`
            : `${state[typename].sortField} asc`)
          : '';

        // const res = updateOrderBy(state[typename].orderBy, newValue);
        // console.log('res', res);
        state[typename].orderBy = newValue;
      }
    }
  }
});

export const sortInfo = (state: RootState, typename: string) => state.sortInfo[typename];
export const { setSortInfoInternal } = sortInfoSlice.actions;
export default sortInfoSlice.reducer;
