import { GetProfiles } from '@lib/smAPI/Profiles/ProfilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetProfiles = createAsyncThunk('cache/getGetProfiles', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetProfiles');
    const response = await GetProfiles();
    console.log('Fetched GetProfiles ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


