import { GetIntroPlayLists } from '@lib/smAPI/Custom/CustomCommands';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetIntroPlayLists = createAsyncThunk('cache/getGetIntroPlayLists', async (_: void, thunkAPI) => {
  try {
    Logger.debug('Fetching GetIntroPlayLists');
    const response = await GetIntroPlayLists();
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


