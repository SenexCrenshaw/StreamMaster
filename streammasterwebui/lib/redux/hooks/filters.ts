import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';
import { DataTableFilterMeta } from 'primereact/datatable';

interface SetFiltersPayload {
  value: DataTableFilterMeta;
  key: string;
}

type FiltersState = Record<string, DataTableFilterMeta>;

const initialState: FiltersState = {};

const FiltersSlice = createSlice({
  initialState,
  name: 'Filters',
  reducers: {
    SetFilters: (state, action: PayloadAction<SetFiltersPayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectFilters = createSelector(
  (state: RootState) => state.filters,
  (Filters) => (key: string) => Filters[key] ?? false
);

export const useFilters = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setFilters = (value: DataTableFilterMeta) => {
    dispatch(
      FiltersSlice.actions.SetFilters({
        key,
        value
      })
    );
  };

  const filters = useSelector((state: RootState) => selectFilters(state)(key));

  return { filters, setFilters };
};

export default FiltersSlice.reducer;
