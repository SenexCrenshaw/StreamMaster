import { GetCustomPlayLists } from '@lib/smAPI/CustomPlayLists/CustomPlayListsCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetCustomPlayLists = createAsyncThunk('cache/getGetCustomPlayLists', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetCustomPlayLists');
    const response = await GetCustomPlayLists();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


