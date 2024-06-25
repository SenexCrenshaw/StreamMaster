import { GetIcons } from '@lib/smAPI/Icons/IconsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';
import { Logger } from '@lib/common/logger';


export const fetchGetIcons = createAsyncThunk('cache/getGetIcons', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetIcons');
    const response = await GetIcons();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


