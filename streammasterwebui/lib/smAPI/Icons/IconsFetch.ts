import { GetIcons } from '@lib/smAPI/Icons/IconsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';

export const fetchGetIcons = createAsyncThunk('cache/getGetIcons', async (_: void, thunkAPI) => {
  try {
    const response = await GetIcons();
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});

