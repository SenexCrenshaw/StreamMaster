import { createSelector, createSlice, type PayloadAction } from '@reduxjs/toolkit';
import { useDispatch, useSelector } from 'react-redux';
import { AppDispatch, RootState } from '../store';

interface SetStringValuePayload {
  value: string | undefined;
  key: string;
}

type StringValueState = Record<string, string | undefined>;

const initialState: StringValueState = {};

const stringValueSlice = createSlice({
  initialState,
  name: 'stringValue',
  reducers: {
    SetStringValue: (state, action: PayloadAction<SetStringValuePayload>) => {
      const { key, value } = action.payload;
      state[key] = value;
    }
  }
});

const selectStringValue = createSelector(
  (state: RootState) => state.stringValue,
  (stringValue) => (key: string) => stringValue[key]
);

export const useStringValue = (key: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setStringValue = (value: string | undefined) => {
    dispatch(
      stringValueSlice.actions.SetStringValue({
        key,
        value
      })
    );
  };

  const stringValue = useSelector((state: RootState) => selectStringValue(state)(key));

  return { setStringValue, stringValue };
};

export default stringValueSlice.reducer;
