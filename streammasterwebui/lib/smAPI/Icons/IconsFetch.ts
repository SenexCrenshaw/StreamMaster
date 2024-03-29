import { GetIcons } from '@lib/smAPI/Icons/IconsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import store from '@lib/redux/store';

export const fetchGetIcons = createAsyncThunk('cache/getGetIcons', async (_: void, thunkAPI) => {
  try {
    const test = store.getState().GetIcons;
    if (test.data) return;
    console.log('Fetching GetIcons');
    const response = await GetIcons();
    console.log('Fetched GetIcons ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

