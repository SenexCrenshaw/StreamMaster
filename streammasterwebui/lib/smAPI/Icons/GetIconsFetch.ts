import { GetIcons } from '@lib/smAPI/Icons/IconsCommands';
import { GetIconsRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetIcons = createAsyncThunk('cache/getGetIcons', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetIcons');
    const response = await GetIcons();
    console.log('Fetched GetIcons ',response?.length);
    return { value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ value: undefined, error: error || 'Unknown error' });
  }
});


